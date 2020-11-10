import useSWR, { cache } from "swr"
import client, {
  urlWith,
  get,
  put,
  post,
  useGet,
  useCreatePost,
  useUpdatePut,
  usePostFormData
} from "../../client"

let API = "/api/announcement"

export const useAnnouncementList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useVerifyAnnouncement = ({ id, type }, data) => {
  let url = !!id && !!type ? `${API}/${id}/${type}` : ""
  return useUpdatePut(url, data)
}

export const useAddAnnouncement = (data) => {
  return useCreatePost(API, data)
}

export const useEditAnnouncement = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useChangeAnnouncementOrders = (data) => {
  let url = `${API}/changeorders`
  return useUpdatePut(url, data)
}
