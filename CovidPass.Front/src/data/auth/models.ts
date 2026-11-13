// © 2020 Partners HealthCare Technology & Innovation Department
export interface LoginModel {
  login: string;
  password: string;
}

export interface AuthInfo {
  login?: string;
  isAuth: boolean;
  token?: string;
  expires?: Date;
  firstName?: string;
  lastName?: string;
  email?: string;
}

export interface Config {
  useLookup: boolean;
  sites: string[];
}
