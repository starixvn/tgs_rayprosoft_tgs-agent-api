import {
  urlWith,
  useGet,
  useCreatePost,
  useUpdatePut,
  useDelete
} from "../../client"

const API = "/api/role"

export const useRoleList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useRole = (id) => {
  let url = id ? `${API}/${id}` : ""
  let { data, ...rest } = useGet(url)

  return {
    role: data?.role,
    ...rest
  }
}

export const useAddRole = (data) => {
  let url = data ? API : ""
  return useCreatePost(url, data)
}

export const useEditRole = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useCopyRole = (id, data) => {
  let url = id ? `${API}/${id}/copy` : ""
  return useCreatePost(url, data)
}

export const useDeleteRole = (id) => {
  let url = id ? `/api/role/${id}` : ""
  return useDelete(url)
}

export const useMenus = (fetchData = true) => {
  let url = fetchData ? `${API}/menus` : ""
  let { result, ...rest } = useCreatePost(url, null)
  return {
    menus: resetDisplay(false, result?.menus),
    ...rest
  }
}

const resetDisplay = (isDisplay, menus = []) => {
  return menus.map((menu) => {
    menu.isDisplay = isDisplay
    if (menu.subMenus) {
      menu.subMenus = resetDisplay(isDisplay, menu.subMenus)
    }
    return menu
  })
}
