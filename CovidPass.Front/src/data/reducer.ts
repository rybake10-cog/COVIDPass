// © 2020 Partners HealthCare Technology & Innovation Department
import { combineReducers } from 'redux';

import { loaderReducer } from 'core/loader';

import { authReducer } from './auth/reducer';
import { applicationReducer } from './application/reducer';
import { lookupReducer } from './lookup/reducer';

export const rootReducer = combineReducers({
  loader: loaderReducer,
  auth: authReducer,
  application: applicationReducer,
  lookup: lookupReducer
});
