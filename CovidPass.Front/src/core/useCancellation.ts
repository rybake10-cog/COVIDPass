// © 2020 Partners HealthCare Technology & Innovation Department
import { useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { ActionType } from 'data/actionTypes';

import { createAction } from './redux';

const cancel = createAction<ActionType>(ActionType.CORE_CANCELLATION);

export const useCancellation = (action: ActionType, mod?: string) => {
  const dispatch = useDispatch();
  useEffect(() => {
    return () => {
      dispatch(cancel(action, mod));
    };
  }, [dispatch, action, mod]);
};
