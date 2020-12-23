import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select, Text } from "bumbag"
import { Locales, Trans, useTrans } from "../../i18n"
import { Field } from "../../select/Field"
import { useProviderList } from "./client"
import AlertDialog from "../../dialog/Alert"

const EmptyOption = { label: "", value: "" }

export const Select = ({ required, ...props }) => {
  //TODO: map item to option
  let { data, isLoading, error } = useProviderList()
  //let options = data?.items.map((item) => item) ?? []
  let options = [{ label: "GP", value: "GP" }]

  if (!required) {
    options = [EmptyOption, ...options]
  }

  return (
    <>
      {error && <AlertDialog {...error} />}
      <_Select isLoading={isLoading} options={options} {...props} />
    </>
  )
}

export default Select
