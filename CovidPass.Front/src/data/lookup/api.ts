// © 2020 Partners HealthCare Technology & Innovation Department
import { http } from 'core/http';

import { PatientInfo } from './models';

const baseUrl = 'lookup/';

export function getPatientInfo(login: string, employeeId: string) {
  return http.post<PatientInfo>(baseUrl + 'getPatientInfo', {}, { login, employeeId });
}
