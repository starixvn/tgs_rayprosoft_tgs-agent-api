import {
  urlWith,
  useGet,
  useCreatePost,
  useUpdatePut,
  useDelete
} from "../../client"

const API = "/api/department"

export const useDepartmentList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useAddDepartment = (data) => {
  let url = data ? API : ""
  return useCreatePost(url, data)
}

export const useEditDepartment = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useDeleteDepartment = (id) => {
  let url = id ? `${API}/${id}` : ""
  return useDelete(url)
}
