import useSWR, { cache } from "swr"
import client, {
  urlWith,
  get,
  put,
  post,
  useGet,
  usePut,
  usePostData,
  usePutData,
  usePostFormData
} from "../../client"

let API = "/api/announcement"

export const useAnnouncementList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useVerifyAnnouncement = ({ id, type }, data) => {
  let url = !!id && !!type ? `${API}/${id}/${type}` : ""
  return usePutData(url, data)
}

export const useAddAnnouncement = (data) => {
  return usePostData(API, data)
}

export const useEditAnnouncement = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useChangeAnnouncementOrders = (data) => {
  let url = `${API}/changeorders`
  return usePutData(url, data)
}

export const useChangeAnnouncementStatus = (id, status) => {
  let url = id && Number.isFinite(status) ? `${API}/${id}/status/${status}` : ""

  return usePut(url)
}
