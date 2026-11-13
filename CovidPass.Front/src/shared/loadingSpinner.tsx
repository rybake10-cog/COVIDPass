// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';

import { Icon } from 'shared';
import { ActionType } from 'data/actionTypes';
import { useLoader } from 'core/useLoader';

interface Props {
  actionType: ActionType;
  mod?: string;
}

export const LoadingSpinner: React.FC<Props> = ({ actionType, mod = undefined }) => {
  const item = useLoader(actionType, mod);
  if (item && item.isWait) return <Icon name="spinner" spin></Icon>;
  if (item && item.isError)
    return <Icon name="times" title={Array.isArray(item.error) ? item.error.join('; ') : item.error}></Icon>;
  return null;
};
