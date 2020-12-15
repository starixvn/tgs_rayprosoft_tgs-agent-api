import {
  urlWith,
  useGet,
  usePostData,
  usePutData,
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
  return usePostData(url, data)
}

export const useEditRole = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useCopyRole = (id, data) => {
  let url = id ? `${API}/${id}/copy` : ""
  return usePostData(url, data)
}

export const useDeleteRole = (id) => {
  let url = id ? `/api/role/${id}` : ""
  return useDelete(url)
}

export const useMenus = (formData) => {
  let url = formData ? `${API}/menus` : ""
  let { result, ...rest } = usePostData(url, formData)
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
