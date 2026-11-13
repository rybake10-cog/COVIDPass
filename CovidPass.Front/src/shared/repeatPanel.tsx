// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';

import { Button, Icon, Flex } from 'shared';
import { ActionType } from 'data/actionTypes';
import { useLoader } from 'core/useLoader';

interface Props {
  actionType: ActionType;
  action: () => any;
  mod?: string;
}

export const RepeatPanel: React.FC<Props> = ({ actionType, action, mod = undefined, children }) => {
  const item = useLoader(actionType, mod);
  if (item && item.isWait)
    return (
      <Flex justifyContent="center" alignItems="center">
        <Icon name="spinner" spin></Icon> Loading...
      </Flex>
    );
  if (item && item.isError)
    return (
      <Flex direction="column" justifyContent="center" alignItems="center">
        <div>{item.error}</div>
        <div>Please try again by clicking the Retry button.</div>
        <Button onClick={action}>Retry</Button>
      </Flex>
    );
  return <>{children}</>;
};
