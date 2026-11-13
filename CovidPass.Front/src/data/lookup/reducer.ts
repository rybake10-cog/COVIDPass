// © 2020 Partners HealthCare Technology & Innovation Department
import { createReducer } from 'core/redux';
import { ActionType } from 'data/actionTypes';

import { PatientInfo } from './models';

interface LookupState {
  patientInfo?: PatientInfo;
}

const initialState: () => LookupState = () => ({});

export const lookupReducer = createReducer(initialState, {
  [ActionType.LOOKUP_SETPATIENTINFO]: 'patientInfo'
});
