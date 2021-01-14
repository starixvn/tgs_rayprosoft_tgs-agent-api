import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text } from "bumbag"
import { useGameProviderBetLimitList } from "./client"
import { useFormContext, Controller, useFieldArray } from "react-hook-form"
import Checkbox from "../../../checkbox"

export const GameProviderBetLimitCheckboxGroup = ({
  name = "betLimitIds",
  gameProvider = {},
  ...props
}) => {
  let { id: gameProviderId } = gameProvider
  let {
    items: betLimits = [],
    isLoading,
    error,
    ...rest
  } = useGameProviderBetLimitList({
    gameProviderId,
    showAll: true
  })

  return (
    <Set orientation="vertical" width="100%">
      <Text.Block>{gameProvider.name}</Text.Block>
      <Table width="100%">
        <Table.Head>
          <Table.Row>
            <Table.HeadCell></Table.HeadCell>
            <Table.HeadCell>Currency</Table.HeadCell>
            <Table.HeadCell>Bet Limit </Table.HeadCell>
            <Table.HeadCell>Max Bet Per Match</Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {betLimits.map((betLimit) => (
            <BetLimitRow checkboxName={name} {...betLimit} />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const BetLimitRow = ({
  checkboxName,
  id,
  currencyCode,
  currencyName,
  minBet,
  maxBet,
  maxBetPerMatch,
  ...props
}) => {
  let formContext = useFormContext()
  let betLimits = formContext?.watch("betLimits") ?? []
  //TODO: default checked status?
  console.log("TODO: how to determine default bet limit checked status")
  let name = `${checkboxName}.${id}`
  return (
    <Table.Row>
      <Table.Cell>
        <Checkbox name={name} checkboxRef={formContext?.register?.()} />
      </Table.Cell>
      <Table.Cell>{currencyCode}</Table.Cell>
      <Table.Cell>
        {minBet} - {maxBet}
      </Table.Cell>
      <Table.Cell>{maxBetPerMatch}</Table.Cell>
    </Table.Row>
  )
}
