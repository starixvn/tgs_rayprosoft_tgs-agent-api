import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select, Text } from "bumbag"
import { Locales, Trans, useTrans } from "../../i18n"
import { Field } from "../../select/Field"
import { useGameProviderList } from "./client"
import AlertDialog from "../../dialog/Alert"

export const GameProviderSelect = ({
  valueField = "id",
  required,
  ...props
}) => {
  let { data, isLoading, error } = useGameProviderList()
  let options =
    data?.items.map((item) => ({
      label: item.name,
      value: item[valueField]
    })) ?? []

  return (
    <>
      {/* {error && <AlertDialog {...error} />} */}
      <_Select isLoading={isLoading} options={options} {...props} />
    </>
  )
}

export default GameProviderSelect
