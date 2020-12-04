import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Table, Set, Button, Box, Stack, PageWithHeader, TopNav } from "bumbag"
import {
  Switch,
  Route,
  Link,
  useHistory,
  useParams,
  useRouteMatch
} from "react-router-dom"
import BrandHeader from "../Header"
import PlayerPage from "./Page"
import { LoginDialogButton } from "./LoginDialog"

export const PlayerList = ({ brandToken }) => {
  let { brandCode, currency } = useParams()
  let { path, url } = useRouteMatch()

  return (
    <Switch>
      <Route exact path={path}>
        <PageWithHeader header={<BrandHeader />}>
          <Stack padding="major-1">
            <PlayerTable
              brandCode={brandCode}
              brandToken={brandToken}
              currency={currency}
            />
          </Stack>
        </PageWithHeader>
      </Route>
      <Route path={`${path}/:username`}>
        <PlayerPage brandToken={brandToken} />
      </Route>
    </Switch>
  )
}

const PlayerTable = ({ brandCode, currency, brandToken }) => {
  return (
    <Set orientation="vertical">
      <Table width="100%" overflow="auto" isHoverable>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell>Username</Table.HeadCell>
            <Table.HeadCell>VIP</Table.HeadCell>
            <Table.HeadCell>Status</Table.HeadCell>
            <Table.HeadCell>Agent</Table.HeadCell>
            <Table.HeadCell>Actions</Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {players.map((player, index) => (
            <PlayerRow
              key={index}
              brandCode={brandCode}
              currency={currency}
              brandToken={brandToken}
              player={player}
            />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const PlayerRow = ({ player, brandCode, currency, brandToken }) => {
  return (
    <Table.Row>
      <Table.Cell>{player.username}</Table.Cell>
      <Table.Cell>{player.vipLevel}</Table.Cell>
      <Table.Cell>{player.status}</Table.Cell>
      <Table.Cell>{player.hasAgent?.toString()}</Table.Cell>
      <Table.Cell>
        <LoginDialogButton
          brandCode={brandCode}
          currency={currency}
          brandToken={brandToken}
          username={player.username}
        />
      </Table.Cell>
    </Table.Row>
  )
}

export default PlayerTable

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
