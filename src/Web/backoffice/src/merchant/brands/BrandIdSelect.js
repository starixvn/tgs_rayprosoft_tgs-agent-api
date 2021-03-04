import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useBrandList } from "./client"
import { useTrans } from "../../i18n"

export const BrandIdSelect = ({ licenseeId = "", ...props }) => {
  let { data, isLoading } = useBrandList({ showAll: true, licenseeId })

  let options =
    data?.items.map((brand) => ({
      label: brand.code,
      value: brand.id
    })) ?? []

  let placeholder = useBrandPlaceholder()
  return (
    <Select
      isLoading={isLoading}
      options={options}
      placeholder={placeholder}
      {...props}
    />
  )
}

const useBrandPlaceholder = () =>
  useTrans({
    zh: "选择品牌",
    en: "Select Brand"
  })
export default BrandIdSelect
