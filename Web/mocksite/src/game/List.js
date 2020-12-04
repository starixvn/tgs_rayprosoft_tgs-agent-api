import React, { useState, useEffect, useMemo } from "react"
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
import { useGameList, useGameUrl } from "./client"

export const GameList = ({
  brandCode,
  currency,
  username,
  gameProviderCode,
  platform,
  language
}) => {
  let { items: games = [], ...rest } = useGameList(
    { brandCode, username, currency },
    { gameProviderCode, platform, language }
  )

  return useMemo(
    () => (
      <Set orientation="vertical">
        <Table width="100%" overflow="auto" isHoverable>
          <Table.Head>
            <Table.Row>
              <Table.HeadCell>Name</Table.HeadCell>
              <Table.HeadCell>Status</Table.HeadCell>
              <Table.HeadCell>Actions</Table.HeadCell>
            </Table.Row>
          </Table.Head>
          <Table.Body>
            {games.map((game, index) => (
              <GameRow
                key={index}
                brandCode={brandCode}
                currency={currency}
                username={username}
                gameProviderCode={gameProviderCode}
                game={game}
                platform={platform}
                language={language}
              />
            ))}
          </Table.Body>
        </Table>
      </Set>
    ),
    [JSON.stringify(games)]
  )
}

const GameRow = ({
  brandCode,
  currency,
  username,
  gameProviderCode,
  platform,
  language,
  game
}) => {
  return (
    <Table.Row>
      <Table.Cell>{game.name}</Table.Cell>
      <Table.Cell>{game.isactive?.toString()}</Table.Cell>
      <Table.Cell>
        <LaunchGameButton
          game={game}
          brandCode={brandCode}
          currency={currency}
          username={username}
          gameProviderCode={gameProviderCode}
          platform={platform}
          language={language}
        />
      </Table.Cell>
    </Table.Row>
  )
}

const LaunchGameButton = ({
  game,
  brandCode,
  currency,
  username,
  gameProviderCode,
  platform,
  language
}) => {
  let [gameWindow, setGameWin] = useState()
  let [formData, setSubmit] = useState()
  let { url, mutate } = useGameUrl({ brandCode, username, currency }, formData)

  useEffect(() => {
    if (url && gameWindow) {
      gameWindow.location = url
      setGameWin(undefined)
      setSubmit(undefined)
      mutate(undefined, false)
    }
  }, [url])
  return (
    <Button
      onClick={(event) => {
        setSubmit({
          gameProviderCode: gameProviderCode,
          gameCode: game?.code
        })

        let win = window.open()
        setGameWin(win)
      }}
    >
      Launch
    </Button>
  )
}
