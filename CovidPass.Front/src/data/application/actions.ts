// © 2020 Partners HealthCare Technology & Innovation Department
import { createAction } from 'core/redux';
import { ActionType } from 'data/actionTypes';

import { Application, ApplicationSubmit } from './models';

export const setApplication = createAction<Application>(ActionType.APPLICATION_SETAPPLICATION);
export const submitAsync = createAction<ApplicationSubmit>(ActionType.APPLICATION_SUBMITASYNC);
