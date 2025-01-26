export type AnyObject = {
  [k: string]: any;
};

// this doesn't work, and there isn't a way...
// export type AnyEnum<T extends AnyObject> = {
//   [k: string]: T | string;
//   [i: number]: string;
// };
