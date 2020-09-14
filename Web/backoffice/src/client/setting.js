import client from "./index"

export const getMenus = (param) => {
  return client.post("/api/role/menus").then(({ menus = [] }) => {
    return menus
  })
}
