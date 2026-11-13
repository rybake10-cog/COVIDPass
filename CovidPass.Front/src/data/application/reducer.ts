// © 2020 Partners HealthCare Technology & Innovation Department
import { createReducer } from 'core/redux';
import { ActionType } from 'data/actionTypes';

import { Application } from './models';

interface ApplicationState {
  application?: Application;
}

const initialState: () => ApplicationState = () => ({});

export const applicationReducer = createReducer(initialState, {
  [ActionType.APPLICATION_SETAPPLICATION]: 'application'
});
