// © 2020 Partners HealthCare Technology & Innovation Department
import { http } from 'core/http';

import { Application, ApplicationSubmit } from './models';

const baseUrl = 'application';

export function submit(submit: ApplicationSubmit) {
  return http.post<Application>(baseUrl + '/submit', submit);
}
