// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';
import classNames from 'classnames';

import './select.scss';

interface Props extends React.SelectHTMLAttributes<any> {
  view?: 'default';
}

export const Select: React.FC<Props> = ({ view = 'default', name, className, children, ...other }) => (
  <select className={classNames('form-select', 'form-input--' + view, className)} name={name} id={name} {...other}>
    {children}
  </select>
);
