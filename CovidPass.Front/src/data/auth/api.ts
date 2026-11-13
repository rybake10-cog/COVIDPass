// © 2020 Partners HealthCare Technology & Innovation Department
import { map } from 'rxjs/operators';

import { http } from 'core/http';

import { LoginModel, AuthInfo, Config } from './models';

const baseUrl = 'auth';

export function login(model: LoginModel) {
  return http.post<AuthInfo>(baseUrl + '/gettoken', model).pipe(
    map<AuthInfo, AuthInfo>(x => ({ ...x, expires: x.expires != null ? new Date(x.expires) : undefined }))
  );
}

export function getConfig() {
  return http.get<Config>(baseUrl + '/getConfig');
}
