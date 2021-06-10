import { RegisterEntity } from "../model/register_form";

// Just a fake loginAPI
export const isValidReg = (regInfo: RegisterEntity): Promise<boolean> =>
  new Promise((resolve) => {
    setTimeout(() => {
      // mock call
      resolve(regInfo.email === "admin@test.com" && regInfo.name === "admin" && regInfo.password === "test");
    }, 500);
  });