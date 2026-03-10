import { EntityDto } from '@abp/ng.core';

export interface PublicUserProfileDto extends EntityDto<string> {
  userName?: string;
  name?: string;
  surname?: string;
  email?: string;
  fotoUrl?: string;
}
