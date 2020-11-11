import useSWR, { cache } from "swr"
import { urlWith, useGet, useCreatePost, useUpdatePut } from "../../client"

const PaymentAPI = "/api/payment"

export const usePaymentList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentAPI) : ""
  return useGet(url)
}

export const usePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPayment = (data) => {
  return useCreatePost(PaymentAPI, data)
}

export const useUpdatePayment = (id, data) => {
  let url = id ? `${PaymentAPI}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useEnablePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}/enable` : ""
  return useUpdatePut(url, {})
}

export const useDisablePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}/disable` : ""
  return useUpdatePut(url, {})
}

export const usePaymentHandingFee = (id) => {
  let url = id ? `${PaymentAPI}/${id}/handingFee` : ""
  return useGet(url)
}

export const useUpdatePaymentHandingFee = (id, data) => {
  let url = id ? `${PaymentAPI}/${id}/handingFee` : ""
  return useUpdatePut(url, data)
}

export const useChangePaymentOrders = (data) => {
  let url = `${PaymentAPI}/ChangeOrders`
  return useUpdatePut(url, data)
}
