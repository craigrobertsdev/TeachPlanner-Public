import { User } from "./lib/models";

// TODO: Implement authentication
async function getUser(email: string): Promise<User | null> {
  const user: User = { email: email, password: "password", firstName: "John", lastName: "Doe", id: "1" };
  return user;
}
