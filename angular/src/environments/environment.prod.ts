import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44358/',
  redirectUri: baseUrl,
  clientId: 'FAFS_App',
  responseType: 'code',
  scope: 'offline_access FAFS',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'FAFS',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44358',
      rootNamespace: 'FAFS',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
