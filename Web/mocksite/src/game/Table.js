import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
import { Table, Set, Button, Box } from "bumbag"

export const GameTable = ({}) => {
  let { path, url } = useRouteMatch()
  let { playerCode } = useParams()
  return (
    <Set orientation="vertical">
      <Box>Player Code {playerCode}</Box>

      <Table width="100%" overflow="auto" isHoverable>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell>Id</Table.HeadCell>
            <Table.HeadCell>Name</Table.HeadCell>
            <Table.HeadCell>Actions</Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {games.map((game) => (
            <GameRow game={game} />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const GameRow = ({ game }) => {
  return (
    <Table.Row>
      <Table.Cell>{game.id}</Table.Cell>
      <Table.Cell>{game.name}</Table.Cell>
      <Table.Cell>
        <Button>TODO: Launch</Button>
      </Table.Cell>
    </Table.Row>
  )
}

//TODO: get games API
const games = [
  {
    id: "1115",
    vendor: "AG",
    name: "百家乐",
    type: "真人"
  }
]
