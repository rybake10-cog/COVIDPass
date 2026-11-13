// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';
import classNames from 'classnames';

import { ActionType } from 'data/actionTypes';
import { useLoader } from 'core/useLoader';

import './messageView.scss';

interface Props {
  className?: string;
  messages?: string | string[];
  actionType?: ActionType;
  mod?: string;
}

export const MessagesView: React.FC<Props> = ({ className, messages, actionType, mod }) => {
  const item = useLoader(actionType != null ? actionType : ActionType.CORE_NOPE, mod);
  let items = null;
  if (messages != null) items = Array.isArray(messages) && messages.length > 0 ? messages : [messages];
  else if (item && item.isError && item.error != null)
    items = Array.isArray(item.error) && item.error.length > 0 ? item.error : [item.error];

  if (items != null) {
    return (
      <div className={classNames('messageView', className)}>
        <ul>
          {items.map((x, i) => (
            <li key={i}>{x}</li>
          ))}
        </ul>
      </div>
    );
  }
  return null;
};
