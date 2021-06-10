export interface RegisterEntity {
    email: string;
    name: string;
    password: string;
  }
  
  export const createEmptyReg = (): RegisterEntity => ({
    email: "",
    name: "",
    password: ""
  });
  