export interface StateInfo {
  state: string;
  stateCode: string;
  branchCount: number;
}

export interface CityInfo {
  city: string;
  branchCount: number;
}

export interface Branch {
  id: number;
  state: string;
  stateCode: string;
  city: string;
  branchName: string;
  branchCode: string;
  address: string;
  phoneNumber?: string;
  email?: string;
  isActive: boolean;
}
