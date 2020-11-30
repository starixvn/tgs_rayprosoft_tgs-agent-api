import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button } from "bumbag"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"

export const PlayerTable = () => {
  return (
    <Set orientation="vertical">
      <Table width="100%" overflow="auto" isHoverable>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell>Code</Table.HeadCell>
            <Table.HeadCell>Username</Table.HeadCell>
            <Table.HeadCell>VIP</Table.HeadCell>
            <Table.HeadCell>Status</Table.HeadCell>
            <Table.HeadCell>Agent</Table.HeadCell>
            <Table.HeadCell>Actions</Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {players.map((player) => (
            <PlayerRow player={player} />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const PlayerRow = ({ player }) => {
  let { path, url } = useRouteMatch()
  return (
    <Table.Row>
      <Table.Cell>{player.playerCode}</Table.Cell>
      <Table.Cell>{player.username}</Table.Cell>
      <Table.Cell>{player.vipLevel}</Table.Cell>
      <Table.Cell>{player.status}</Table.Cell>
      <Table.Cell>{player.hasAgent?.toString()}</Table.Cell>
      <Table.Cell>
        <Button use={Link} to={`/${player.playerCode}`}>
          Actions
        </Button>
      </Table.Cell>
    </Table.Row>
  )
}

const players = [
  {
    playerCode: "4ucaggtg8jap",
    username: "TP01_cny_en-us",
    vipLevel: "名称E",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtg9lkf",
    username: "TP02_cny_en-us",
    vipLevel: "名称C",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtg9ucj",
    username: "TP01_cny_zh-cn",
    vipLevel: "名称C",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtga2ui",
    username: "RP01_cny_en-us",
    vipLevel: "名称C",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtgabic",
    username: "RP02_cny_en-us",
    vipLevel: "名称C",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtgarxd",
    username: "JC9Yrk3xYAPm",
    vipLevel: "名称B",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtgb12k",
    username: "tSaIC3AYDOVz",
    vipLevel: "名称A",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtgba3y",
    username: "cTGnjubMfpaO",
    vipLevel: "名称C",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtgbj3r",
    username: "Rde3BZ7TaTMC",
    vipLevel: "名称A",
    status: "Enabled",
    hasAgent: false
  },
  {
    playerCode: "4ucaggtgbrow",
    username: "NumIDAFtm5xq",
    vipLevel: "名称A",
    status: "Enabled",
    hasAgent: false
  }
]
