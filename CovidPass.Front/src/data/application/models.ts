// © 2020 Partners HealthCare Technology & Innovation Department
export interface Application {
  noSymptoms: boolean;
  firstName: string;
  lastName: string;
  email: string;
  loginOrEmployeeId: string;
  date: Date;
  created: Date;
}

export interface ApplicationSubmit {
  employeeId: string;
  login: string;
  lastName: string;
  firstName: string;
  email: string;
  noSymptoms?: boolean;
  influenza?: boolean;
  fever?: boolean;
  cough?: boolean;
  shortnessOfBreath?: boolean;
  soreThroat?: boolean;
  diarrheaNausea?: boolean;
  newLossOfSmell?: boolean;
  runnyNose?: boolean;
  muscleAche?: boolean;
  isManual: boolean;
  site: string;
  initials: string;
  language: string;
}
