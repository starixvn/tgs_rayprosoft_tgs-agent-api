import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useLicenseeList } from "./client"
import { useTrans } from "../../i18n"

export const LicenseeIdSelect = ({ resellerId = "", ...props }) => {
  let { data, isLoading } = useLicenseeList({ showAll: true, resellerId })

  let options =
    data?.items.map((licensee) => ({
      label: licensee.code,
      value: licensee.id
    })) ?? []

  let placeholder = useLicenseePlaceholder()
  return (
    <Select
      isLoading={isLoading}
      options={options}
      placeholder={placeholder}
      {...props}
    />
  )
}

const useLicenseePlaceholder = () =>
  useTrans({
    zh: "选择公司",
    en: "Select Licensee"
  })

export default LicenseeIdSelect
