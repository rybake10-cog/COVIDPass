// © 2020 Partners HealthCare Technology & Innovation Department
import { map } from 'rxjs/operators';
import { combineEpics } from 'redux-observable';

import { createEpic } from 'core/epic';

import { getPatientInfoAsync, setPatientInfo } from './actions';
import { getPatientInfo } from './api';

const getPatientInfoEpic = createEpic(getPatientInfoAsync, data =>
  getPatientInfo(data.login, data.employeeId).pipe(map(response => setPatientInfo(response)))
);

export const lookupEpic = combineEpics(getPatientInfoEpic);
