import client from "./index"

export const getMenus = (param) => {
  return client.get("/api/setting", { key: "menus" }).then(({ items = [] }) => {
    let settings = items?.[0].value ?? ""
    return JSON.parse(settings)
  })
}
