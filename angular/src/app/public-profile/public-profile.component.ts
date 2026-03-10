import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { UserProfileService } from '../proxy/users/user-profile.service';
import { PublicUserProfileDto } from '../proxy/users/models';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-public-profile',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './public-profile.component.html',
    styleUrls: ['./public-profile.component.scss']
})
export class PublicProfileComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private userProfileService = inject(UserProfileService);

    user: PublicUserProfileDto | null = null;
    loading = true;
    error: string | null = null;

    ngOnInit(): void {
        const userId = this.route.snapshot.paramMap.get('id');
        if (userId) {
            this.loadPublicProfile(userId);
        } else {
            this.error = 'No se proporcionó un ID de usuario.';
            this.loading = false;
        }
    }

    loadPublicProfile(id: string) {
        this.userProfileService.getPublicProfile(id)
            .pipe(finalize(() => this.loading = false))
            .subscribe({
                next: (data) => this.user = data,
                error: (err) => {
                    this.error = 'No se pudo cargar el perfil del usuario.';
                    console.error(err);
                }
            });
    }
}
