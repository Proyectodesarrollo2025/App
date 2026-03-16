import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService } from '../proxy/account/profile.service';
import { UserProfileService } from '../proxy/users/user-profile.service';
import { DestinationRatingService, DestinationService } from '@proxy/destinations';
import { DestinationRatingDto, DestinationDto } from '@proxy/application/contracts/destinations/models';
import { AuthService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-my-profile',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './my-profile.component.html',
    styleUrls: ['./my-profile.component.scss'],
})
export class MyProfileComponent implements OnInit {
    private fb = inject(FormBuilder);
    private profileService = inject(ProfileService);
    private userProfileService = inject(UserProfileService);
    private authService = inject(AuthService);
    private toaster = inject(ToasterService);
    private confirmation = inject(ConfirmationService);
    private router = inject(Router);
    private ratingService = inject(DestinationRatingService);
    private destinationService = inject(DestinationService);

    profileForm: FormGroup;
    passwordForm: FormGroup;
    loading = false;
    passwordLoading = false;
    fotoUrl: string = '';
    avatarLoading = false;
    myRatings: any[] = [];
    ratingsLoading = false;

    ngOnInit(): void {
        this.buildProfileForm();
        this.buildPasswordForm();
        this.loadProfile();
        this.loadMyRatings();
    }

    buildProfileForm() {
        this.profileForm = this.fb.group({
            userName: [{ value: '', disabled: true }],
            name: ['', [Validators.required, Validators.maxLength(64)]],
            surname: ['', [Validators.required, Validators.maxLength(64)]],
            email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
            phoneNumber: [''],
        });
    }

    buildPasswordForm() {
        this.passwordForm = this.fb.group({
            currentPassword: ['', [Validators.required]],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required]],
        }, { validators: this.passwordMatchValidator });
    }

    passwordMatchValidator(g: FormGroup) {
        const newPass = g.get('newPassword')?.value;
        const confirmPass = g.get('confirmPassword')?.value;
        return newPass === confirmPass ? null : { mismatch: true };
    }

    loadProfile() {
        this.profileService.get().subscribe((profile) => {
            this.profileForm.patchValue(profile);

            // Cargar foto de perfil desde nuestro servicio extendido
            this.userProfileService.getMyProfile().subscribe(res => {
                this.fotoUrl = res.fotoUrl || 'assets/images/default-avatar.png';
            });
        });
    }

    onFileSelected(event: any) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e: any) => {
                const base64Image = e.target.result;
                this.uploadAvatar(base64Image);
            };
            reader.readAsDataURL(file);
        }
    }

    uploadAvatar(base64Image: string) {
        this.avatarLoading = true;
        this.userProfileService.updateProfilePicture({ fotoUrl: base64Image })
            .pipe(finalize(() => this.avatarLoading = false))
            .subscribe(() => {
                this.fotoUrl = base64Image;
                this.toaster.success('Foto de perfil actualizada', 'Éxito');
            });
    }

    updateProfile() {
        if (this.profileForm.invalid) return;

        this.loading = true;

        // Guardar información básica
        this.profileService.update(this.profileForm.getRawValue())
            .subscribe(() => {
                // Si hay una foto cargada, nos aseguramos de guardarla también al dar clic en el botón
                if (this.fotoUrl && this.fotoUrl !== 'assets/images/default-avatar.png') {
                    this.userProfileService.updateProfilePicture({ fotoUrl: this.fotoUrl })
                        .pipe(finalize(() => this.loading = false))
                        .subscribe(() => {
                            this.toaster.success('Perfil y foto actualizados correctamente', 'Éxito');
                        });
                } else {
                    this.loading = false;
                    this.toaster.success('Perfil actualizado correctamente', 'Éxito');
                }
            });
    }

    changePassword() {
        if (this.passwordForm.invalid) return;

        this.passwordLoading = true;
        const { currentPassword, newPassword } = this.passwordForm.getRawValue();
        this.profileService.changePassword({ currentPassword, newPassword })
            .pipe(finalize(() => this.passwordLoading = false))
            .subscribe(() => {
                this.toaster.success('Contraseña actualizada correctamente', 'Éxito');
                this.passwordForm.reset();
            });
    }

    deleteAccount() {
        this.confirmation.warn(
            '¿Estás seguro de que deseas eliminar tu cuenta? Esta acción no se puede deshacer.',
            'Eliminar Cuenta',
            { messageLocalizationParams: [] }
        ).subscribe((status: Confirmation.Status) => {
            if (status === Confirmation.Status.confirm) {
                this.userProfileService.deleteMyAccount().subscribe(() => {
                    this.toaster.success('Tu cuenta ha sido eliminada correctamente.', 'Cuenta Eliminada');
                    this.authService.logout().subscribe(() => {
                        this.router.navigate(['/account/login']);
                    });
                });
            }
        });
    }

    loadMyRatings() {
        this.ratingsLoading = true;
        this.ratingService.getMyRatings()
            .pipe(finalize(() => this.ratingsLoading = false))
            .subscribe(ratings => {
                this.myRatings = ratings;
                // Intentar cargar nombres de destinos si es necesario, 
                // pero por ahora mostraremos los IDs o lo que tengamos
                this.loadDestinationNames();
            });
    }

    loadDestinationNames() {
        this.myRatings.forEach(rating => {
            this.destinationService.get(rating.destinationId).subscribe(dest => {
                rating.destinationName = dest.city || dest.name;
            });
        });
    }

    deleteRating(id: string) {
        this.confirmation.warn(
            '¿Estás seguro de que deseas eliminar esta reseña?',
            'Eliminar Reseña'
        ).subscribe(status => {
            if (status === Confirmation.Status.confirm) {
                this.ratingService.deleteRating(id).subscribe(() => {
                    this.toaster.success('Reseña eliminada');
                    this.loadMyRatings();
                });
            }
        });
    }
}
