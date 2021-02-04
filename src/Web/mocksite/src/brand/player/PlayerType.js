import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Stack, DropdownMenu } from "bumbag"

const Test = "Test" //2
const Real = "Real" //1

const options = [Test, Real]

export const PlayerType = {
  Real,
  Test
}

export const intValue = (playerType = Test) => {
  if (playerType === Real) {
    return 1
  }

  return 2 //Test
}

export const PlayerTypeDropdownMenu = ({ value, ...props }) => {
  return (
    <DropdownMenu
      menu={
        <>
          {options.map((playerType) => (
            <DropdownMenu.Item
              key={playerType}
              onClick={(event) => {
                props?.onChange(playerType)
              }}
            >
              {playerType}
            </DropdownMenu.Item>
          ))}
        </>
      }
    >
      <Button iconAfter="chevron-down">{value ? value : "Player Type"}</Button>
    </DropdownMenu>
  )
}

export default PlayerType
