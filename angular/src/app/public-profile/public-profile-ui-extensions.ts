import { EntityAction } from '@abp/ng.components/extensible';
import { eIdentityComponents } from '@abp/ng.identity';
import { IdentityUserDto } from '@abp/ng.identity/proxy';
import { Router } from '@angular/router';

const viewPublicProfileAction = new EntityAction<IdentityUserDto>({
  text: 'Ver Perfil Público',
  action: data => {
    const router = data.getInjected(Router);
    router.navigate(['/public-profile', data.record.id]);
  },
  icon: 'fa fa-user-circle',
});

export const PUBLIC_PROFILE_ENTITY_ACTION_CONTRIBUTORS = {
  [eIdentityComponents.Users]: [
    (actionList: any) => actionList.addTail(viewPublicProfileAction)
  ],
};

export const PUBLIC_PROFILE_ENTITY_PROP_CONTRIBUTORS = {
  [eIdentityComponents.Users]: [
    (propList: any) => {
      // Función para identificar qué columnas borrar (FotoUrl y Preferencias)
      const shouldRemove = (prop: any) => {
        if (!prop || !prop.name) return false;
        const name = prop.name.toLowerCase();
        return name.includes('foto') || name.includes('pref');
      };

      // Removemos todas las que coincidan
      let removed;
      do {
        removed = propList.remove(shouldRemove);
      } while (removed);
    },
  ],
};
