import { urlWith, useGet, usePostData, usePutData } from "../../client"

const API = "/api/users"

export const useAdminList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useAddAdmin = (data) => {
  let url = data ? API : ""
  return usePostData(url, data)
}

export const useEditAdmin = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useEditPassword = (id, data) => {
  let url = id ? `${API}/${id}/password` : ""
  return usePutData(url, data)
}
