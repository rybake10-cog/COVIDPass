// © 2020 Partners HealthCare Technology & Innovation Department
import { useSelector } from 'react-redux';

import { findLoaderItem } from 'core/loader';
import { ActionType } from 'data/actionTypes';
import { StoreType } from 'core/store';

export const useLoader = (actionType: ActionType, mod?: string) => {
  return useSelector((state: StoreType) => findLoaderItem(state.loader, actionType, mod));
};
