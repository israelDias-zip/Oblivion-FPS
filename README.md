# Oblivion FPS — Projeto Unity separado

Este é um projeto Unity **independente** do `Oblivium-main` (o protótipo 2D
top-down original). Ele nasce a partir dele — mesma versão do Unity, mesmas
configurações de projeto — mas com os Assets reduzidos ao que faz sentido
pra a versão em primeira pessoa/3D.

- **Versão do Unity:** 2022.3.62f3 (mesma do projeto original — importante
  abrir com essa versão ou mais nova pra evitar prompts de upgrade).
- **Como abrir:** Unity Hub → "Add project from disk" → selecione esta pasta.

## O que veio

`ProjectSettings/` e `Packages/` — copiados do projeto original (tags como
"Player", eixos de input, física, etc. já configurados).

`Assets/Scripts/`:
- `AudioManager.cs`, `SanitySystem.cs` — copiados sem alteração (não
  dependiam de nada 2D).
- `PlayerController.cs`, `MouseLook.cs`, `CameraEffects.cs`, `Flashlight.cs`,
  `FlashlightHUD.cs` — novos, para o controle em primeira pessoa.
- `SanityHUD.cs`, `GameController.cs`, `MazeExit.cs`, `WrongPathTrigger.cs`
  — adaptados de 2D para 3D.

## O que ficou de fora (de propósito)

Tudo isso é específico da versão 2D e não faz sentido aqui — mas continua
disponível no `Oblivium-main` se quiser reaproveitar algo depois:
- Pacotes de arte 2D (`Cainos` — pixel art top-down, `RPGW_Caves`).
- A cena antiga (`SampleScene.unity`), o prefab do jogador 2D
  (`Oblivion.prefab`) e o `PlayerAnimator.controller`.
- `MapMemorySystem.cs` (o "esquecimento" do mapa) e o `FogTile.asset` — são
  baseados em Tilemap 2D; vão precisar de um redesenho pra 3D.
- `MazeBuilderTool.cs` / `OblivionSetup.cs` (pasta `Editor/`) — ferramentas
  do editor pra gerar o labirinto em Tilemap.

## Primeiro passo ao abrir

O projeto não tem nenhuma cena ainda — crie uma (**File → New Scene →
Basic (Built-in)**, salve como `FPS_Prototype.unity`) e siga o mesmo passo a
passo de montagem (bloco de teste → Player com CharacterController → câmera
com MouseLook/CameraEffects → lanterna → HUD) que já passamos antes.
