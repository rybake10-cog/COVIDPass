// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';
import classNames from 'classnames';

import './input.scss';

interface Props extends React.InputHTMLAttributes<any> {
  view?: 'login' | 'default';
}

export const Input: React.FC<Props> = ({ view = 'default', name, className, ...other }) => (
  <input className={classNames('form-input', 'form-input--' + view, className)} name={name} id={name} {...other} />
);
