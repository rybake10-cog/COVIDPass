// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';

import { ActionType } from 'data/actionTypes';
import { useLoader } from 'core/useLoader';
import { Icon, Button } from 'shared';

import { ButtonProps } from './button';

interface Props extends ButtonProps {
  actionType: ActionType;
  mod?: string;
  disabled?: boolean;
}

export const LoadingButton: React.FC<Props> = ({ actionType, mod = undefined, children, disabled, ...other }) => {
  const item = useLoader(actionType, mod);
  const content = item && item.isWait ? <Icon spin name="spinner" /> : children;
  const isDisabled = (item && item.isWait) || disabled;

  return (
    <Button disabled={isDisabled} {...other}>
      {content}
    </Button>
  );
};
