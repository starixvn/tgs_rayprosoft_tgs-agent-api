import { urlWith, useGet, useCreatePost, useUpdatePut } from "../../client"

const API = "/api/users"

export const useAdminList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useAddAdmin = (data) => {
  let url = data ? API : ""
  return useCreatePost(url, data)
}

export const useEditAdmin = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useEditPassword = (id, data) => {
  let url = id ? `${API}/${id}/password` : ""
  return useUpdatePut(url, data)
}
