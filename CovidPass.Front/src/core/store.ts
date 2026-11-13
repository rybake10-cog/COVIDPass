// © 2020 Partners HealthCare Technology & Innovation Department
import { createStore, compose, applyMiddleware } from 'redux';
import { createEpicMiddleware } from 'redux-observable';

import { rootReducer } from 'data/reducer';
import { rootEpic } from 'data/epic';
import { setAuthInfo } from 'data/auth/actions';

const composeEnhancers = (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose; // add support for Redux dev tools

const epicMiddleware = createEpicMiddleware<any, any, any>();

export const store = createStore(rootReducer, composeEnhancers(applyMiddleware(epicMiddleware)));

epicMiddleware.run(rootEpic);

const authInfo = localStorage.getItem('authInfo');
if (authInfo != null) store.dispatch(setAuthInfo(JSON.parse(authInfo)));
export interface StoreType extends ReturnType<typeof rootReducer> {} // eslint-disable-line @typescript-eslint/no-empty-interface
