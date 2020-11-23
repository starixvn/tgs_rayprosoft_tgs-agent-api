import {
  urlWith,
  useGet,
  usePostData,
  usePutData,
  useDelete
} from "../../client"

const API = "/api/department"

export const useDepartmentList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useAddDepartment = (data) => {
  let url = data ? API : ""
  return usePostData(url, data)
}

export const useEditDepartment = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useDeleteDepartment = (id) => {
  let url = id ? `${API}/${id}` : ""
  return useDelete(url)
}
