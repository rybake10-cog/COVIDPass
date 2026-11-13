// © 2020 Partners HealthCare Technology & Innovation Department
import './button.scss';

import React from 'react';
import classNames from 'classnames';

export interface ButtonProps extends React.ButtonHTMLAttributes<any> {
  view?: 'default' | 'clear' | 'green' | 'red' | 'link';
}

export const Button: React.FC<ButtonProps> = ({ className, view = 'default', children, ...other }) => {
  return (
    <button className={classNames('button', view, className)} {...other}>
      {children}
    </button>
  );
};
