import client from "./index"

export const roles = (searchParams = {}) => {
  return client.get("/api/role", { searchParams })
}
