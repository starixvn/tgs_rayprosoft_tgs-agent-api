import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, FieldStack } from "bumbag"
import { Trans, useTrans } from "../i18n"
import { NormalLabel } from "../form/Label"
import { useFormContext, useFieldArray } from "react-hook-form"
import { GameProviderCheckboxGroup } from "../game/providers/GameProviderCheckboxGroup"

export const GameProviderFieldsBox = ({ readonly }) => {
  let { watch, control } = useFormContext()

  let fieldsName = "gameProviders"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let gameProviders = watch(fieldsName)

  return (
    gameProviders && (
      <>
        <NormalLabel>
          <Trans zh="游戏厂商" en="Game Provider" />
        </NormalLabel>
        <GameProviderCheckboxGroup
          fieldArrayContext={fieldArrayContext}
          disabled={readonly}
        />
      </>
    )
  )
}

export default GameProviderFieldsBox
