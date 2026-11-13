// © 2020 Partners HealthCare Technology & Innovation Department
import { createAction } from 'core/redux';
import { ActionType } from 'data/actionTypes';

import { PatientInfo } from './models';

export const getPatientInfoAsync = createAction<{ login: string; employeeId: string }>(
  ActionType.LOOKUP_GETPATIENTINFOASYNC
);
export const setPatientInfo = createAction<PatientInfo>(ActionType.LOOKUP_SETPATIENTINFO);
