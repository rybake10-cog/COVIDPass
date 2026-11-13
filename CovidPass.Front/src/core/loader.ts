// © 2020 Partners HealthCare Technology & Innovation Department
import { ActionType } from 'data/actionTypes';

import { AppAction } from './baseTypes';
import { createReducer, createAction } from './redux';

interface LoaderSubState {
  [mod: string]: LoaderData;
}

interface LoaderState {
  [actionType: string]: LoaderData | LoaderSubState;
}

interface LoaderData {
  id: ActionType;
  mod?: string;
  isOk: boolean;
  isWait: boolean;
  isError: boolean;
  error?: string | string[];
}

export function findLoaderItem(loaderState: LoaderState, type: ActionType, mod?: string): LoaderData | undefined {
  if (mod != null) {
    const stateForType = loaderState[type];
    return stateForType != null ? (stateForType as LoaderSubState)[mod] : undefined;
  }
  return loaderState[type] as LoaderData;
}

export const ok = (id: any, mod?: string) =>
  createAction<LoaderData>(ActionType.CORE_LOADER)({ id, mod, isOk: true, isWait: false, isError: false });

export const wait = (id: any, mod?: string) =>
  createAction<LoaderData>(ActionType.CORE_LOADER)({ id, mod, isOk: false, isWait: true, isError: false });

export const error = (id: any, error: string | string[], mod?: string) =>
  createAction<LoaderData>(ActionType.CORE_LOADER)({
    id,
    mod,
    isOk: false,
    isWait: false,
    isError: true,
    error
  });

const calcState = (state: LoaderState, action: AppAction<LoaderData>): LoaderState => {
  if (action.data.mod != null)
    return {
      ...state,
      [action.data.id]: {
        ...state[action.data.id],
        [action.data.mod]: action.data
      }
    };
  return {
    ...state,
    [action.data.id]: action.data
  };
};

export const loaderReducer = createReducer<LoaderState>(() => ({}), {
  [ActionType.CORE_CANCELLATION]: (state, action: AppAction<ActionType>) =>
    calcState(state, ok(action.data, action.mod)),
  [ActionType.CORE_LOADER]: calcState
});
