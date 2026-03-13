import { ExtensibleObject } from '@abp/ng.core';

export interface ProfileDto extends ExtensibleObject {
    userName?: string;
    email?: string;
    name?: string;
    surname?: string;
    phoneNumber?: string;
    isExternal?: boolean;
    hasPassword?: boolean;
}

export interface UpdateProfileDto extends ExtensibleObject {
    userName?: string;
    email?: string;
    name?: string;
    surname?: string;
    phoneNumber?: string;
}

export interface ChangePasswordInput {
    currentPassword?: string;
    newPassword?: string;
}
