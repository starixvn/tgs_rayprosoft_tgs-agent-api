import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../../select/Select"
import { Locales, Trans, useTrans } from "../../../i18n"
import { useGameProviderConfigurationList } from "./client"

export const GameProviderConfigSelect = ({ gameProviderId, ...props }) => {
  let { items = [], isLoading, error } = useGameProviderConfigurationList({
    gameProviderId,
    showAll: true
  })

  let options = items?.map(({ name, id, ...item }) => ({
    label: name,
    value: id
  }))

  //TODO: need defaultValue = item.isDefault = true?
  //TODO: placeholder?
  return <Select options={options} isLoading={isLoading} {...props} />
}
